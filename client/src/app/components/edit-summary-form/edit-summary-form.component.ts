import { TitleCasePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { Summary } from 'src/app/models/Summary';
import { KeywordService } from 'src/app/services/keyword.service';
import { SubjectService } from 'src/app/services/subject.service';
import { SummaryService } from 'src/app/services/summary.service';
import { ValidateFileExtension } from 'src/app/validators/validators';

@Component({
  selector: 'app-edit-summary-form',
  templateUrl: './edit-summary-form.component.html',
  styleUrls: ['./edit-summary-form.component.css']
})
export class EditSummaryFormComponent implements OnInit, OnChanges {

  @Input() selectedSummary?: Summary;

  isLoading: boolean = false;
  backendError?: string;

  allSubjects: string[] = [];
  subjects: string[] = [];

  imageFileName?: string | null;
  imageFormData?: FormData | null;

  allAutocompleteKeywords: string[] = [];
  autocompleteKeywords: string[] = [];
  keywords: string[] = [];

  @Output() summaryEdited = new EventEmitter<Summary>();
  @Output() summaryDeleted = new EventEmitter<Summary>();

  editSummaryForm: FormGroup = this.fb.group({
    semester: ['', [Validators.required, Validators.min(1), Validators.max(10)]],
    subject: ['', [Validators.required, Validators.pattern(/^[^&?]*$/)]],
    name: ['', [Validators.required, Validators.pattern(/^[^&?]*$/)]],
    image: ['', [ValidateFileExtension]]
  });

  keywordForm: FormGroup = this.fb.group({
    keyword: ['', Validators.required]
  });

  constructor(
    private fb: FormBuilder,
    private titlecasePipe: TitleCasePipe,
    private subjectService: SubjectService, 
    private keywordService: KeywordService,
    private summaryService: SummaryService
  ) { }

  ngOnInit(): void {
    this.subjectService.get().subscribe((subjects: string[]) => {
      this.allSubjects = subjects;
      this.subjects = subjects;
    })

    this.keywordService.get().subscribe((keywords: string[]) => {
      this.allAutocompleteKeywords = keywords;
      this.autocompleteKeywords = keywords.map((keyword: string) => this.titlecasePipe.transform(keyword));
    })
  }

  ngOnChanges(changes: SimpleChanges): void {
    const newSummary: Summary = changes['selectedSummary'].currentValue;
    if (newSummary) {
      this.editSummaryForm.setValue({
        semester: newSummary.semester,
        subject: newSummary.subject.charAt(0).toUpperCase() + newSummary.subject.slice(1),
        name: newSummary.name,
        image: null
      });

      this.keywords = newSummary.keywords;
      this.imageFileName = null;
      this.imageFormData = null;
    }
  }

  onFileChange(event: any) {
    this.imageFileName = event.target.files[0].name;
    const file: File = event.target.files[0];
    const formData: FormData = new FormData();
    formData.append('image', file)
    this.imageFormData = formData;
  }

  onSubjectChange(): void {
    this.subjects = this.allSubjects.filter((subject: string) => {
      return subject.startsWith(this.editSummaryForm.value.subject ? this.editSummaryForm.value.subject : '');
    });
  }

  onKeywordChange(): void {
    this.autocompleteKeywords = this.allAutocompleteKeywords.filter((keyword: string) => {
      return keyword.startsWith(this.keywordForm.value.keyword ?  this.keywordForm.value.keyword.toLowerCase() : '');
    }).map((keyword: string) => this.titlecasePipe.transform(keyword));
  }

  onKeywordSubmit(form: FormGroupDirective): void {
    if (this.keywordForm.valid && !this.keywords.includes(this.keywordForm.value.keyword)) {
      this.keywords.unshift(this.keywordForm.value.keyword);
      form.resetForm();
      this.autocompleteKeywords = this.allAutocompleteKeywords.map((keyword: string) => this.titlecasePipe.transform(keyword));
    }
  }

  onKeywordDelete(keyword: string): void {
    this.keywords = this.keywords.filter((k: string) => k !== keyword)
  }

  deleteImage(id: number): Promise<void> {
    return new Promise((resolve, reject) =>  {
      this.summaryService.deleteImage(id).subscribe({
        next: () => resolve(),
        error: (error: HttpErrorResponse) => reject(error)
      })
    })
  }

  async onUpdate() {

    if (this.editSummaryForm.valid) {
      const summary: Summary = {
        semester: this.editSummaryForm.value.semester,
        subject: this.editSummaryForm.value.subject,
        name: this.editSummaryForm.value.name,
        keywords: this.keywords.map((keyword: string) => keyword.toLowerCase()),
      }

      this.isLoading = true;
      // If new Image Selected, at first delte Old Image
      if (this.editSummaryForm.value.image) {
        try {
          await this.deleteImage(this.selectedSummary!.id!);
        } catch (error) {
          this.backendError = 'Cloud not delete old Image.'
          this.isLoading = false;
          return;
        }
      }

      this.summaryService.update(this.selectedSummary!.id!, summary).subscribe({
        next: (summary: Summary) => {
          if (this.editSummaryForm.value.image) {
            this.summaryService.upload(this.selectedSummary!.id!, this.imageFormData!).subscribe({
              next: (summary: Summary) => {
                summary.isImageLoading = true;
                this.summaryEdited.emit(summary);
                this.imageFileName = null;
                this.imageFormData = null;
                this.backendError = undefined;
                this.isLoading = false;
              },
              error: (error: HttpErrorResponse) => {
                console.log(error);
                if (typeof error.error === 'object') this.backendError = 'An error occoured.'
                else this.backendError = error.error;
                summary.isImageLoading = true;
                this.summaryEdited.emit(summary);
                this.isLoading = false;
              }
            })
          } else {
            this.isLoading = false;
            this.backendError = undefined;
          }
        },
        error: (error: HttpErrorResponse) => {
          console.log(error);
          if (typeof error.error === 'object') this.backendError = 'An error occoured.'
          else this.backendError = error.error;
          this.isLoading = false;
        }
      })
    }
  }

  onDelete(form: FormGroupDirective, subForm: FormGroupDirective): void {
    if (this.editSummaryForm.valid) {
      this.isLoading = true;
      this.summaryService.delete(this.selectedSummary!.id!).subscribe({
        next: () => {
          this.summaryDeleted.emit(this.selectedSummary);
          this.imageFileName = null;
          this.imageFormData = null;
          this.keywords = [];
          subForm.resetForm();
          form.resetForm();
          this.backendError = undefined;
          this.isLoading = false;
        },
        error: (error: HttpErrorResponse) => {
          console.log(error);
          if (typeof error.error === 'object') this.backendError = 'An error occoured.'
          else this.backendError = error.error;
          this.isLoading = false;
        }
      })
    }
  }
}

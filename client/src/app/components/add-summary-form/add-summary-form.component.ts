import { TitleCasePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormGroupDirective, Validators } from '@angular/forms';
import { Summary } from 'src/app/models/Summary';
import { KeywordService } from 'src/app/services/keyword.service';
import { SubjectService } from 'src/app/services/subject.service';
import { SummaryService } from 'src/app/services/summary.service';

function ValidateFileExtension(control: AbstractControl) {
  if (control.value) {
    const splitted = control.value.split('.');
    const extension = splitted[splitted.length - 1];
  
    if (!(extension === 'jpg')) {
      return { invalidFileExtension: true };
    }
  }
  return null;
}

@Component({
  selector: 'app-add-summary-form',
  templateUrl: './add-summary-form.component.html',
  styleUrls: ['./add-summary-form.component.css']
})
export class AddSummaryFormComponent implements OnInit {

  isLoading: boolean = false;
  backendError?: string;

  allSubjects: string[] = [];
  subjects: string[] = [];

  imageFileName?: string | null;
  imageFormData?: FormData | null;

  allAutocompleteKeywords: string[] = [];
  autocompleteKeywords: string[] = [];
  keywords: string[] = [];

  @Output() summaryAdded = new EventEmitter<Summary>();

  addSummaryForm: FormGroup = this.fb.group({
    semester: ['', [Validators.required, Validators.min(1), Validators.max(10)]],
    subject: ['', Validators.required],
    name: ['', Validators.required],
    image: ['', [Validators.required, ValidateFileExtension]]
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
      this.subjects = subjects.map((keyword: string) => keyword.charAt(0).toUpperCase() + keyword.slice(1));
    })

    this.keywordService.get().subscribe((keywords: string[]) => {
      this.allAutocompleteKeywords = keywords;
      this.autocompleteKeywords = keywords.map((keyword: string) => keyword.charAt(0).toUpperCase() + keyword.slice(1));
    })
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
      return subject.startsWith(this.addSummaryForm.value.subject ? this.addSummaryForm.value.subject : '');
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
      this.autocompleteKeywords = this.allAutocompleteKeywords.map((keyword: string) => keyword.charAt(0).toUpperCase() + keyword.slice(1));
    }
  }

  onDelete(keyword: string): void {
    this.keywords = this.keywords.filter((k: string) => k !== keyword)
  }

  onSubmit(form: FormGroupDirective, subForm: FormGroupDirective): void {
    if (this.addSummaryForm.valid) {
      const summary: Summary = {
        semester: this.addSummaryForm.value.semester,
        subject: this.addSummaryForm.value.subject,
        name: this.addSummaryForm.value.name,
        keywords: this.keywords.map((keyword: string) => keyword.toLowerCase()),
      }

      this.isLoading = true;
      this.summaryService.add(summary).subscribe({
        next: (id: number) => {
          // Update Keywords and Subjects
          summary.keywords.forEach((keyword: string) => {
            this.allAutocompleteKeywords.indexOf(keyword) === -1 && this.allAutocompleteKeywords.push(keyword);
          })
          this.allSubjects.indexOf(summary.subject) === -1 && this.allSubjects.push(summary.subject);
          // Upload Image
          this.summaryService.upload(id, this.imageFormData!).subscribe({
            next: (summary: Summary) => {
              summary.isImageLoading = true;
              this.summaryAdded.emit(summary);
              this.imageFileName = null;
              this.imageFormData = null;
              this.keywords = [];
              subForm.resetForm();
              form.resetForm();
              this.onKeywordChange();
              this.onSubjectChange();
              this.backendError = undefined;
              this.isLoading = false;
            },
            error: (error: HttpErrorResponse) => {
              console.log(error);
              if (typeof error.error === 'object') this.backendError = 'An error occoured.'
              else this.backendError = error.error;
              summary.isImageLoading = true;
              this.summaryAdded.emit(summary);
              this.isLoading = false;
            }
        })},
        error: (error: HttpErrorResponse) => {
          console.log(error);
          if (typeof error.error === 'object') this.backendError = 'An error occoured.'
          else this.backendError = error.error;
          this.isLoading = false;
        }
      });
    }
  }
}

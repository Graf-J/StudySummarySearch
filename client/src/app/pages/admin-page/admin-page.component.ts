import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { Summary } from 'src/app/models/Summary';
import { SummaryService } from 'src/app/services/summary.service';
import { DropboxTreeComponent } from 'src/app/components/dropbox-tree/dropbox-tree.component';
import { Subject } from 'rxjs';
import { MatTabGroup } from '@angular/material/tabs';

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.css']
})
export class AdminPageComponent implements OnInit {

  @ViewChild('tabGroup') private tabGroup!: MatTabGroup;
  @ViewChild(DropboxTreeComponent) dropboxTreeComponent!: DropboxTreeComponent;

  semester?: number;
  subject?: string;
  name?: string;
  keyword?: string;
  authorId?: number;
  
  isLargeDisplay: boolean = window.innerWidth >= 1100;

  summarySubject: Subject<Summary> = new Subject();
  summary?: Summary;
  summaries: Summary[] = [];

  constructor(private summaryService: SummaryService) { 
    this.summarySubject.subscribe((summary: Summary) => {
      this.summary = summary;
    });
  }

  ngOnInit(): void {
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.isLargeDisplay = event.target.innerWidth >= 992;
  }

  querySummaries(): void {
    this.summaryService.get(this.semester, this.subject, this.keyword, this.name, this.authorId).subscribe((summaries: Summary[]) => {
      this.summaries = summaries;
    })
  }

  onNameSelect(data: { semester: number | undefined, subject: string | undefined, name: string | undefined }) {
    const { semester, subject, name } = data;
    this.semester = semester;
    this.subject = subject;
    this.name = name;
    this.keyword = undefined;
    this.authorId = undefined;

    this.querySummaries();
  }

  onSearch(data: { keyword: string | undefined, authorId: number | undefined }) {
    this.keyword = data.keyword;
    this.authorId = data.authorId;
    this.semester = undefined;
    this.subject = undefined;
    this.name = undefined;

    this.querySummaries();
  }

  onSummaryAdded(summary: Summary): void {
    this.summaries.unshift(summary);
    this.dropboxTreeComponent.reset();
  }

  onSummaryEdited(summary: Summary): void {
    this.summaries = this.summaries.filter((s: Summary) => s.id !== summary.id);
    this.summaries.unshift(summary);
    this.dropboxTreeComponent.reset();
  }

  onSummaryDeleted(summary: Summary): void {
    this.summaries = this.summaries.filter((s: Summary) => s.id !== summary.id);
    this.dropboxTreeComponent.reset();
  }

  onImageClick(summary: Summary) {
    this.summarySubject.next(summary);
    this.tabGroup.selectedIndex = 1;
  }
}

import { Component, HostListener, OnInit } from '@angular/core';
import { SummaryService } from 'src/app/services/summary.service';
import { Summary } from 'src/app/models/Summary';

@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css']
})
export class LandingPageComponent implements OnInit {
  semester?: number;
  subject?: string;
  name?: string;
  keyword?: string;
  authorId?: number;

  isLargeDisplay: boolean = window.innerWidth >= 992;

  summaries: Summary[] = [];

  constructor(private summaryService: SummaryService) { }

  ngOnInit(): void { }

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
}

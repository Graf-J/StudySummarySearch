import { TitleCasePipe } from '@angular/common';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/models/User';
import { KeywordService } from 'src/app/services/keyword.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-search-field',
  templateUrl: './search-field.component.html',
  styleUrls: ['./search-field.component.css']
})
export class SearchFieldComponent implements OnInit {

  allKeywords: string[] = [];
  keywords: string[] = [];
  authors: User[] = [];
  authorUsernames: string[] = [];

  keyword: string = '';
  authorUsername: string = '';
  author?: User;

  @Output() search = new EventEmitter<{ keyword: string | undefined, authorId: number | undefined }>();

  constructor(
    private titlecasePipe: TitleCasePipe,
    private keywordService: KeywordService, 
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.keywordService.get(this.keyword).subscribe((keywords: string[]) => {
      this.allKeywords = keywords;
      this.keywords = keywords.map((keyword: string) => this.titlecasePipe.transform(keyword));
    })

    this.userService.get().subscribe((users: User[]) => {
      this.authors = users;
      this.authorUsernames = this.authors.map((user: User) => user.username);
    })
  }

  onSubmit(): void {
    this.search.emit({
      keyword: this.allKeywords.find((keyword: string) => keyword == this.keyword.toLowerCase()), 
      authorId: this.authors.find((author: User) => author.username === this.authorUsername)?.id
    });
  }

  onKeywordChange(): void {
    this.keywords = this.allKeywords.filter((keyword: string) => {
      return keyword.startsWith(this.keyword.toLowerCase())
    }).map((keyword: string) => this.titlecasePipe.transform(keyword));
  }

  onAuthorChange(): void {
    this.authorUsernames = this.authors.filter((author: User) => { 
      return author.username.startsWith(this.authorUsername) 
    }).map((author: User) => author.username);
  }
}

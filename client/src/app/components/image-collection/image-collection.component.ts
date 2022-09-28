import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Summary } from 'src/app/models/Summary';
import { ImageDialogComponent } from '../image-dialog/image-dialog.component';

@Component({
  selector: 'app-image-collection',
  templateUrl: './image-collection.component.html',
  styleUrls: ['./image-collection.component.css']
})
export class ImageCollectionComponent implements OnInit {

  @Input() display!: string;
  @Input() summaries: Summary[] = [];

  @Output() imageClick = new EventEmitter<Summary>();

  isLoading: boolean = true;

  constructor(public dialog: MatDialog) { }

  ngOnInit(): void { }

  getCssWrapperClass(): string {
    switch (this.display) {
      case 'desktop':
        return 'img-collection-wrapper';
      case 'mobile':
        return 'img-collection-wrapper-mobile';
      case 'admin-desktop':
        return 'img-collection-wrapper-admin';
      case 'admin-mobile':
        return 'img-collection-wrapper-admin-mobile';
      default:
        return 'img-collection-wrapper';
    }
  }

  getCssLoadContainerClass(): string {
    if (this.display === 'desktop' || this.display === 'admin-desktop') return 'img-load-container';
    return 'img-load-container-mobile';
  }

  getCssImageClass(): string {
    if (this.display === 'desktop' || this.display === 'admin-desktop') return 'image';
    return 'image-mobile';
  }

  onClick(summary: Summary): void {
    if (this.display === 'admin-desktop' || this.display === 'admin-mobile') {
      this.imageClick.emit(summary);
    } else {
      this.dialog.open(ImageDialogComponent, { data: summary });
    }
  }

  onLoaded(index: number): void {
    this.summaries[index].isImageLoading = false;
  }
}

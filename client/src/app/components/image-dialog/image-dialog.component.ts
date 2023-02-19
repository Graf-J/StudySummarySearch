import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Summary } from 'src/app/models/Summary';

@Component({
  selector: 'app-image-dialog',
  templateUrl: './image-dialog.component.html',
  styleUrls: ['./image-dialog.component.css']
})
export class ImageDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public summary: Summary) { }

  ngOnInit(): void { }

  onClick(base64Image: string | undefined) {
    let data = `data:image/jpeg;base64,${base64Image}`;
    let w = window.open('about:blank');
    let image = new Image();
    image.src = data;
    setTimeout(function(){
      w!.document.write(image.outerHTML);
    }, 0);
  }
}

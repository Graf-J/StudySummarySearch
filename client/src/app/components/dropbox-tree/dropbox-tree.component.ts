import { CollectionViewer, SelectionChange } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, Injectable, OnInit, Output, EventEmitter } from '@angular/core';
import { BehaviorSubject, map, merge, Observable } from 'rxjs';
import { SemesterService } from 'src/app/services/semester.service';
import { SubjectService } from 'src/app/services/subject.service';
import { NameService } from 'src/app/services/name.service';

export class DynamicFlatNode {
  constructor(
    public item: string, 
    public level = 1, 
    public expandable = false, 
    public prevNode?: DynamicFlatNode, 
    public isLoading = false
  ) {}
}

@Injectable()
export class DynamicDataSource {

  dataChange = new BehaviorSubject<DynamicFlatNode[]>([]);

  get data(): DynamicFlatNode[] { return this.dataChange.value; }
  set data(value: DynamicFlatNode[]) {
    this._treeControl.dataNodes = value;
    this.dataChange.next(value);
  }

  constructor(
    private _treeControl: FlatTreeControl<DynamicFlatNode>, 
    private subjectService: SubjectService,
    private nameService: NameService
  ) {}

  connect(collectionViewer: CollectionViewer): Observable<DynamicFlatNode[]> {
    this._treeControl.expansionModel.changed.subscribe(change => {
      if (
        (change as SelectionChange<DynamicFlatNode>).added ||
        (change as SelectionChange<DynamicFlatNode>).removed
      ) {
        this.handleTreeControl(change as SelectionChange<DynamicFlatNode>);
      }
    });

    return merge(collectionViewer.viewChange, this.dataChange).pipe(map(() => this.data));
  }

  disconnect(collectionViewer: CollectionViewer): void {}

  handleTreeControl(change: SelectionChange<DynamicFlatNode>) {
    if (change.added) {
      change.added.forEach(node => this.toggleNode(node, true));
    }
    if (change.removed) {
      change.removed.slice().reverse().forEach(node => this.toggleNode(node, false));
    }
  }

  toggleNode(node: DynamicFlatNode, expand: boolean) {
    const index = this.data.indexOf(node);
    if (!expand)
    {
      let count = 0;
        for (let i = index + 1; i < this.data.length
          && this.data[i].level > node.level; i++, count++) {}
        this.data.splice(index + 1, count);
        this.dataChange.next(this.data);
    }
    else
    {
    node.isLoading = true;
    switch (node.level) {
      case 0:
        this.subjectService.get(parseInt(node.item.split(' ')[1])).subscribe(subjects => {
          const nodes = subjects.map(subject => new DynamicFlatNode(subject, 1, true, node));
          this.data.splice(index + 1, 0, ...nodes);
          node.isLoading = false;
          this.dataChange.next(this.data);
        })
        break;
      case 1:
        this.nameService.get(parseInt(node.prevNode!.item.split(' ')[1]), node.item).subscribe(names => {
          const nodes = names.map(name => new DynamicFlatNode(name, 2, false, node));
          this.data.splice(index + 1, 0, ...nodes);
          node.isLoading = false;
          this.dataChange.next(this.data);
        })
        break;
      }
    }
  }
}

@Component({
  selector: 'app-dropbox-tree',
  templateUrl: './dropbox-tree.component.html',
  styleUrls: ['./dropbox-tree.component.css']
})
export class DropboxTreeComponent implements OnInit {

  rootNodesLoading: boolean = true;

  @Output() nameSelect = new EventEmitter<{ semester: number | undefined, subject: string | undefined, name: string | undefined }>();

  constructor(
    private semesterService: SemesterService, 
    private subjectService: SubjectService, 
    private nameService: NameService
  ) { }

  ngOnInit()
  {
    this.treeControl = new FlatTreeControl<DynamicFlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new DynamicDataSource(this.treeControl, this.subjectService, this.nameService);
    this.getSemesters();
  }
  treeControl!: FlatTreeControl<DynamicFlatNode>;

  dataSource!: DynamicDataSource;

  getLevel = (node: DynamicFlatNode) => node.level;

  isExpandable = (node: DynamicFlatNode) => node.expandable;

  hasChild = (_: number, _nodeData: DynamicFlatNode) => _nodeData.expandable;

  getSemesters() {
    this.rootNodesLoading = true;
    this.semesterService.get().subscribe(res =>{
      this.dataSource!.data = res.map(semester => new DynamicFlatNode(`Semester ${ semester }`, 0, true));
      this.rootNodesLoading = false;
    })
  }

  reset() {
    this.ngOnInit();
  }

  onClick(node: DynamicFlatNode) {
    let semester: number | undefined = undefined;
    let subject: string | undefined = undefined;
    let name: string | undefined = undefined;
    switch (node.level) {
      case 0:
        semester = parseInt(node.item.split(' ')[1]);
        break;
      case 1:
        semester = parseInt(node.prevNode!.item.split(' ')[1]);
        subject = node.item;
        break;
      case 2:
        semester = parseInt(node.prevNode!.prevNode!.item.split(' ')[1]);
        subject = node.prevNode!.item;
        name = node.item;
        break;
      default:
        break;
    }

    this.nameSelect.emit({ semester, subject, name })
  }
}

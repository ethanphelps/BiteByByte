import { Component, OnInit } from '@angular/core';

import { Category } from "@app/_models/category";

/*
 * Component for Category and FocusArea classes
 */
@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.scss']
})
export class CategoryComponent implements OnInit {
  private category: Category;

  constructor() { }

  ngOnInit(): void {
  }

}

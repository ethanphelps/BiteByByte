import { Component } from '@angular/core';

import { User } from '@app/_models/user';
import { AccountService } from '@app/_services/account.service';
import { CategoryComponent } from "@app/_components/category/category.component";
import { CategoryService } from "@app/_services/category.service";

@Component({
  templateUrl: 'home.component.html',
  styleUrls: ['home.component.scss']
})
export class HomeComponent {
  user: User;
  categories: CategoryComponent[];

  constructor(
    private accountService: AccountService,
    private categoryService: CategoryService
  ) {
    this.user = this.accountService.userValue;
  }

  // DEBUG: test authenticated get request
  getUsers() {
   this.accountService.getAll().subscribe((data) =>{
     console.log(data);
   });
  }

  // DEBUG: test if logged in
  amILoggedIn() {
    console.log(this.accountService.userValue);
  }

  // DEBUG
  logBearer() {
    console.log(this.accountService.userValue['Username']);
    console.log(this.accountService.userValue['Token']);
  }
}

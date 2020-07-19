import { Component } from '@angular/core';

import { User } from '@app/_models/user';
import { AccountService } from '@app/_services/account.service';

@Component({ templateUrl: 'home.component.html' })
export class HomeComponent {
  user: User;

  constructor(private accountService: AccountService) {
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
}

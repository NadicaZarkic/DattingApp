import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './account.service';
import { User } from './_models/user';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
 
  title = 'The Dating app';
  users:any;
  url="https://localhost:5001/api/users";

   constructor(private accountService:AccountService,private presence:PresenceService){
    
   }

  ngOnInit()
 {
  
  this.setCurrentUser();
 
 }

 setCurrentUser() {
  const user: User = JSON.parse(localStorage.getItem('user'));
  if(user)
  {
    this.accountService.setCurrentUser(user);
    this.presence.createHubConnection(user);
  }
  
 }

 
}

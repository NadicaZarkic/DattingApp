import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-erors',
  templateUrl: './test-erors.component.html',
  styleUrls: ['./test-erors.component.css']
})
export class TestErorsComponent implements OnInit {

  baseUrl = 'https://localhost:5001/api/';
  validationErrors: string[] =[];

  constructor(private http:HttpClient) { }

  ngOnInit(): void {
  }

  get404Error() {
    return this.http.get(this.baseUrl+'buggy/not-found').subscribe(
      response => {
        console.log(response);
      },error =>{
        console.log(error);
      })
  }


get400Error() {
  return this.http.get(this.baseUrl+'buggy/bad-request').subscribe(
    response => {
      console.log(response);
    },error =>{
      console.log(error);
    })
  }


get500Error() {
  return this.http.get(this.baseUrl+'buggy/server-error').subscribe(
    response => {
      console.log(response);
    },error =>{
      console.log(error);
    })
}


get401Error() {
  return this.http.get(this.baseUrl+'buggy/auth').subscribe(
    response => {
      console.log(response);
    },error =>{
      console.log(error);
    })
}

get400ValidationError() {
  return this.http.post(this.baseUrl+'account/register',{}).subscribe(
    response => {
      console.log(response);
    },error =>{
      this.validationErrors = error;
    })
}

}
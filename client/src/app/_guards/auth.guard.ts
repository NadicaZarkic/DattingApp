import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { map, Observable } from 'rxjs';
import { AccountService } from '../account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountService:AccountService,private toastr:ToastrService,private router:Router){

  }
  canActivate(): Observable<boolean>  {
    return this.accountService.currentUser$.pipe(
      map(user =>{
        if (user) {return true;}
        else{
        this.router.navigateByUrl('/')
        this.toastr.error('You do not have permission for this action.');
        return false;
        }
      })
    )
  }
  
}

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import {NgxGalleryOptions} from '@kolkov/ngx-gallery';
import {NgxGalleryImage} from '@kolkov/ngx-gallery';
import {NgxGalleryAnimation} from '@kolkov/ngx-gallery'
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccountService } from 'src/app/account.service';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit,OnDestroy {
  @ViewChild('memberTabs', {static:true}) memberTabs: TabsetComponent;
  member:Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];
  user?:User;

  constructor(private accountService:AccountService, private route:ActivatedRoute,
    private messageService: MessageService,
    public presenceService:PresenceService,
    private router:Router)
   { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user =>{
        if(user) this.user=user;
      }
    });
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
   }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  ngOnInit(): void {
    this.route.data.subscribe(data =>{
      this.member = data.member;
    })

  

    this.route.queryParams.subscribe(params=>{
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })

    this.galleryOptions = [{
      width:'500px',
      height:'500px',
      imagePercent:100,
      thumbnailsColumns:4,
      imageAnimation:NgxGalleryAnimation.Slide,
      preview:false
    }]

    this.galleryImages = this.getImages();

   
  }

  getImages():NgxGalleryImage[] {
    const imageUrls = [];
    for(const photo of this.member.photos) {
      imageUrls.push({
        small:photo?.url,
        medium:photo?.url,
        big:photo?.url
      })
    }
    return imageUrls;
  }



  loadMessages()
  {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }

  selectTab(tabId:number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data:TabDirective)
  {
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.user)
    {
      this.messageService.createHubConnection(this.user,this.member.username);
    }
    else
    {
      this.messageService.stopHubConnection();
    }
  }

}

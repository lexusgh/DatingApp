import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage , NgxGalleryAnimation} from '@kolkov/ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';
@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private userService: UserService, private alertifyService: AlertifyService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    // this.loadUser();
    this.route.data.subscribe(data => {
      this.user = data.user;
    });

    this.route.queryParams.subscribe(params => {
      const selectedTab = params.tab > 0 ? params.tab : 0;
     
      this.memberTabs.tabs[selectedTab].active = true;
    });

    this.galleryOptions =
        [
      {
          width: '500px',
          height: '500px',
          imagePercent: 80,
          thumbnailsColumns: 4,
          imageAnimation: NgxGalleryAnimation.Slide,
          preview: false
      },

     ];

    this.galleryImages = this.getImage();
  }



// loadUser() {
// this.userService.getUser(+this.route.snapshot.params.id).subscribe((user: User) => {
// this.user = user;
//   }, error => {
//     this.alertifyService.error(error);

//   });

// }
getImage() {
  const imageUrl = [];

  for (const photo of this.user.photos) {
    imageUrl.push({
      small: photo.url,
      medium: photo.url,
      large: photo.url,
      description: photo.url
    });
  }
  return imageUrl;

}

selectTab(tabId: number) {
  this.memberTabs.tabs[tabId].active = true;
}
}

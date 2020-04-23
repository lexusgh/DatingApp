import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';




@Injectable()

export class MemberEditResolver implements Resolve<User> {
    constructor(private userService: UserService, private router: Router,
                private alertifyService: AlertifyService,
                private authService: AuthService) {}

    resolve(router: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertifyService.error('Problem retrieving your data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
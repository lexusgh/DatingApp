import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Injectable } from '@angular/core';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';




@Injectable()

export class MemberDetailResolver implements Resolve<User> {
    constructor(private userService: UserService, private router: Router,
                private alertifyService: AlertifyService) {}

    resolve(router: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(router.params.id).pipe(
            catchError(error => {
                this.alertifyService.error('Problem retrieving data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}

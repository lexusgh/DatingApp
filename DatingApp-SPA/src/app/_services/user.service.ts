import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class UserService {
baseUrl = environment.apiUrl + 'users/' ;

constructor(private http: HttpClient) { }

getUsers(page?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>> {
  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
  let params = new HttpParams();
  if (page != null && itemsPerPage != null) {
    params = params.append('pageNumber', page);
    params = params.append('pageSize', itemsPerPage);

  }

  if (userParams != null){
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderedBy', userParams.orderBy);
    

  }
  return this.http.get<User[]>(this.baseUrl + 'getusers', {observe: 'response', params}).
  pipe(
    map(response => {
      paginatedResult.result = response.body;
      if (response.headers != null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}


getUser(id): Observable<User> {

  return this.http.get<User>(this.baseUrl + 'getuser/' + id);
}

updateUser(id: number, user: User) {
  return this.http.put(this.baseUrl + 'updateuser/' + id, user);
}

setMainPhoto(userid: number, id: number) {
  return this.http.post(this.baseUrl  + userid + '/photos/' + id + '/setmain', {});
}

deletePhoto(userid: number, id: number) {
  return this.http.delete(this.baseUrl  + userid + '/photos/' + id );
}
}

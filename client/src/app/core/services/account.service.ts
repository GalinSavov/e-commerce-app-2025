import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { User } from '../../shared/models/user';
import { Address } from '../../shared/models/address';
import { LoginRequest } from '../../shared/models/loginRequest';
import { RegisterRequest } from '../../shared/models/registerRequest';
import { map, catchError, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private baseURL = environment.apiURL;
  currentUser = signal<User | null>(null);
  login(values:LoginRequest){
    let params = new HttpParams();
    params = params.append('useCookies',true);
    return this.http.post<User>(this.baseURL + 'login',values,{params});
  }
  register(values:RegisterRequest){
    return this.http.post(this.baseURL + 'account/register',values)
  }
  getUserInfo(){
    return this.http.get<User>(this.baseURL + 'account/user-info').pipe(
      map(user =>{
        this.currentUser.set(user);
        return user;
      }))
  }
  logout(){
    return this.http.post(this.baseURL + 'account/logout',{});
  }
  updateAddress(address:Address){
    return this.http.post(this.baseURL + 'account/address',address).pipe(
      tap(() => {
        this.currentUser.update(user => {
          if(user) user.address = address;
          return user;
        })
      })
    )
  }
  getAuthState(){
    return this.http.get<{isAuthenticated:boolean}>(this.baseURL + 'account/auth-status');
  }
}

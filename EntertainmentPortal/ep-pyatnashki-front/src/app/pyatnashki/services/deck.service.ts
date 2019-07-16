import { AccountService } from 'src/app/account/services/account.service';
import { Jwt } from './../../account/models/jwt';
import { ConfigService } from './../../shared/services/config.service';
import { Deck } from './../models/deck';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DeckService {

  private url = 'https://localhost:44380/api/deck';
  constructor(private http: HttpClient, private accountService: AccountService) {
   }




  newDeck() {
  // tslint:disable-next-line: max-line-length
    return this.http.post<Deck>(this.url, this.accountService.getUserInfo(), {headers: new HttpHeaders ({
      Authorization: this.accountService.getAuthorizationHeaderValue(),
      "Email": this.accountService.getEmailnHeaderValue()
    }) , withCredentials: true});
  }

  moveTile(num: number) {
// tslint:disable-next-line: max-line-length
    return this.http.put<Deck>(this.url, num, {headers: new HttpHeaders ({
      Authorization: this.accountService.getAuthorizationHeaderValue(),
      "Email": this.accountService.getEmailnHeaderValue()
    }) , withCredentials: true});
  }

  getDeck() {
// tslint:disable-next-line: max-line-length
    return this.http.get<Deck>(this.url, {headers: new HttpHeaders ({
      Authorization: this.accountService.getAuthorizationHeaderValue(),
      "Email": this.accountService.getEmailnHeaderValue()
    })
      , withCredentials: true});
  }

}

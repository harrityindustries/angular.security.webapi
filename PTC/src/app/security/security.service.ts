import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs';
import { of } from 'rxjs';
import { tap } from 'rxjs/operators';

import { AppUserAuth } from './app-user-auth';
import { AppUser } from './app-user';

const API_URL = 'http://localhost:5000/api/security/';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root'
})
export class SecurityService {
  securityObject: AppUserAuth = new AppUserAuth();

  constructor(private httpClient: HttpClient) { }

  login(entity: AppUser): Observable<AppUserAuth> {
    // Initializse security object
    this.resetSecurityObject();

    return this.httpClient.post<AppUserAuth>(API_URL + 'login', entity, httpOptions).pipe(
      tap(resp => {
        Object.assign(this.securityObject, resp);
        localStorage.setItem('bearerToken', this.securityObject.bearerToken);
      })
    );
  }

  logout(): void {
    this.resetSecurityObject();
  }

  resetSecurityObject(): void {
    this.securityObject.userName = '';
    this.securityObject.bearerToken = '';
    this.securityObject.isAuthenticated = false;

    this.securityObject.claims = [];

    localStorage.removeItem('bearerToken');
  }

  // This method can be called a couple of different ways
  // *hasClaim="'claimType'"  // Assumes claimValue is true
  // *hasClaim="'claimType:value'"  // Compares claimValue to value
  // *hasClaim="['claimType1','claimType2:value','claimType3']"
  hasClaim(claimType: any, claimValue?: any) {
    let ret = false;

    if (typeof claimType === 'string') {
      ret = this.isClaimValid(claimType, claimValue);
    } else {
      const claims: Array<string> = claimType;
      if (claims) {
        for (let index = 0; index < claims.length; index++) {
          ret = this.isClaimValid(claims[index]);
          if (ret) {
            break;
          }
        }
      }
    }
    return ret;
  }

  private isClaimValid(claimType: string, claimValue?: string): boolean {
    let ret = false;
    let auth: AppUserAuth = null;

    auth = this.securityObject;

    if (auth) {
      // See if the claim type has a value
      // *hasClaim="'claimType:value'"
      if (claimType.indexOf(':') >= 0) {
        const words: Array<string> = claimType.split(':');
        claimType = words[0].toLowerCase();
        claimValue = words[1];
      } else {
        claimType = claimType.toLowerCase();
        claimValue = claimValue ? claimValue : 'true';
      }

      ret = auth.claims.find(c => c.claimType.toLowerCase() === claimType && c.claimValue === claimValue) != null;

      return ret;
    }
  }
}

import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { SecurityService } from './security.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  /**
   *
   */
  constructor(private securityService: SecurityService, private router: Router) {
  }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
      const claimType = next.data['claimType'];
      if (this.securityService.securityObject.isAuthenticated === true && this.securityService.hasClaim(claimType)) {
        return true;
      } else {
        this.router.navigate(['login'], { queryParams: { returnUrl: state.url }});
      }
  }
}

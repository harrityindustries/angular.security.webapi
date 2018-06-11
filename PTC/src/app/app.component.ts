import { Component } from '@angular/core';
import { AppUserAuth } from './security/app-user-auth';
import { SecurityService } from './security/security.service';

@Component({
  selector: 'ptc-root',
  templateUrl: './app.component.html'
})

export class AppComponent {
  securityObject: AppUserAuth = null;
  title = 'Ng Security w/ Web Tokens';

  constructor(private securityService: SecurityService) {
    this.securityObject = this.securityService.securityObject;
  }

  logout(): void {
    this.securityService.logout();
  }
}

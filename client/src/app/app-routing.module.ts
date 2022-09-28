import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPageComponent } from './pages/admin-page/admin-page.component';
import { AdminUsersPageComponent } from './pages/admin-users-page/admin-users-page.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { AdminAuthGuard } from './services/admin-auth.guard';
import { SuperUserGuard } from './services/super-user.guard';

const routes: Routes = [
  { path: "", component: LandingPageComponent },
  { path: "admin", component: AdminPageComponent, canActivate: [AdminAuthGuard] },
  { path: "admin/login", component: LoginPageComponent },
  { path: "admin/users", component: AdminUsersPageComponent, canActivate: [SuperUserGuard] },
  { path: "**", pathMatch: "full", redirectTo: "" }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

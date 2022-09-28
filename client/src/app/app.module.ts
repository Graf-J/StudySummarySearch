import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AuthInterceptor } from './services/auth.interceptor';
import { TitleCasePipe } from '@angular/common';

import { MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDialogModule } from '@angular/material/dialog';

import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { DropboxTreeComponent } from './components/dropbox-tree/dropbox-tree.component';
import { SearchFieldComponent } from './components/search-field/search-field.component';
import { ImageCollectionComponent } from './components/image-collection/image-collection.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { AdminPageComponent } from './pages/admin-page/admin-page.component';
import { AddSummaryFormComponent } from './components/add-summary-form/add-summary-form.component';
import { EditSummaryFormComponent } from './components/edit-summary-form/edit-summary-form.component';
import { AdminUsersPageComponent } from './pages/admin-users-page/admin-users-page.component';
import { ImageDialogComponent } from './components/image-dialog/image-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    LandingPageComponent,
    DropboxTreeComponent,
    SearchFieldComponent,
    ImageCollectionComponent,
    LoginPageComponent,
    AdminPageComponent,
    AddSummaryFormComponent,
    EditSummaryFormComponent,
    AdminUsersPageComponent,
    ImageDialogComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    MatTreeModule,
    MatIconModule,
    MatButtonModule,
    MatProgressBarModule,
    MatSidenavModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatDialogModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    TitleCasePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

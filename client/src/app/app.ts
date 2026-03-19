import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Nav } from "../layout/nav/nav";
import { Router, RouterOutlet } from '@angular/router';
import { ConfirmDialogComponent } from "../shared/confirm-dialog/confirm-dialog.component";

@Component({
  selector: 'app-root',
  imports: [Nav, RouterOutlet, ConfirmDialogComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {  private http = inject(HttpClient);
  protected router = inject(Router);

}

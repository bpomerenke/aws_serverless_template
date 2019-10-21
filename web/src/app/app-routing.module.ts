import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MessageDashboardComponent } from './components/message-dashboard/message-dashboard.component';

const routes: Routes = [
  { path: '**', component: MessageDashboardComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

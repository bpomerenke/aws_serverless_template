import { Component, OnInit } from '@angular/core';
import { Message } from 'src/app/models/message';
import { ApiService } from 'src/app/services/api.service';

@Component({
  selector: 'app-message-dashboard',
  templateUrl: './message-dashboard.component.html',
  styleUrls: ['./message-dashboard.component.scss']
})
export class MessageDashboardComponent implements OnInit {

  messages: Message[];
  messageError: string;

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.getMessages().subscribe(messages => {
      this.messages = messages;
    }, 
    (err) => { this.messageError = err; });
  }

}

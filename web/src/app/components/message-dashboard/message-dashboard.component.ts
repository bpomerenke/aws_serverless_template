import { Component, OnInit } from '@angular/core';
import { Message } from 'src/app/models/message';
import { ApiService } from 'src/app/services/api.service';
import { WebsocketService } from 'src/app/services/websocket.service';

@Component({
  selector: 'app-message-dashboard',
  templateUrl: './message-dashboard.component.html',
  styleUrls: ['./message-dashboard.component.scss']
})
export class MessageDashboardComponent implements OnInit {

  messages: Message[];
  messageError: string;

  constructor(private apiService: ApiService, private wsService: WebsocketService) { }

  ngOnInit() {
    this.apiService.getMessages().subscribe(messages => {
      this.messages = messages;
      this.wsService.connect().subscribe(message => {
        console.log('got message: ', message);
      })
    }, 
    (err) => { this.messageError = err; });
  }

}

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

  curMessage: string;
  sendingMessage = false;

  constructor(private apiService: ApiService, private wsService: WebsocketService) { }

  ngOnInit() {
    this.apiService.getMessages().subscribe(messages => {
      this.messages = messages.sort((a,b) => { return b.timestamp.localeCompare(a.timestamp)});
      this.wsService.connect().subscribe(message => {
        messages.unshift(message);
      })
    }, 
    (err) => { this.messageError = err; });
  }

  sendMessage(){
    this.sendingMessage = true;
    this.apiService.sendMessage(this.curMessage).subscribe(() => {
      this.curMessage = '';
      this.sendingMessage = false;
    });
  }

}

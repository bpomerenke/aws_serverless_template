import { Injectable } from '@angular/core';
import { AppConfigService } from '../app-config.service';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  private socket;
  private subject;

  private NORMAL_CLOSURE_CODE = 1000;
  
  constructor(private appConfig: AppConfigService) { }

  connect() : Observable<any> {
    this.socket = new WebSocket(this.appConfig.getWsUrl());
    this.socket.onmessage = event => {
      this.subject.next(JSON.parse(event.data));
    };

    this.socket.onclose = event => {
      this.subject.complete();
    };

    this.subject = new Subject<any>();
    return this.subject;
  }

  disconnect() {
    this.socket.close(this.NORMAL_CLOSURE_CODE);
  }
}

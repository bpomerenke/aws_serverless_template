import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AppConfigService } from '../app-config.service';
import { VersionInfo } from '../models/version';
import { Message } from '../models/message';
@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private httpClient: HttpClient, private appConfig: AppConfigService) { }

  getVersion(): Observable<VersionInfo> {
    const baseUrl = this.appConfig.getApiUrl();
    return this.httpClient.get<VersionInfo>(`${baseUrl}/version`);
  }

  getMessages(): Observable<Message[]> {
    const baseUrl = this.appConfig.getApiUrl();
    return this.httpClient.get<Message[]>(`${baseUrl}/messages`);
  }

  sendMessage(message: string): Observable<any> {
    const baseUrl = this.appConfig.getApiUrl();
    const payload = { 
      clientId: 'web', 
      timestamp: new Date().getTime(), 
      msgType: 'Message', 
      msgText: message ,
      msgTime: new Date().getTime(),
      sender: 'web'
    };
    return this.httpClient.post<Message>(`${baseUrl}/messages`, payload);
  }
}
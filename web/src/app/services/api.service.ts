import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AppConfigService } from '../app-config.service';
@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private httpClient: HttpClient, private appConfig: AppConfigService) { }

  getVersion(): Observable<any>
  {
    const baseUrl = this.appConfig.getApiUrl();
    return this.httpClient.get(`${baseUrl}/version`);
  }
}

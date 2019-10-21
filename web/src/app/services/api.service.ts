import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AppConfigService } from '../app-config.service';
import { VersionInfo } from '../models/version';
@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(private httpClient: HttpClient, private appConfig: AppConfigService) { }

  getVersion(): Observable<VersionInfo>
  {
    const baseUrl = this.appConfig.getApiUrl();
    return this.httpClient.get<VersionInfo>(`${baseUrl}/version`);
  }
}

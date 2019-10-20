import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AppConfigService {

  constructor(private http: HttpClient) { }

  appConfig: any;

  loadAppConfig() {
    return this.http.get('/assets/data/appConfig.json?' + new Date().getTime() )
      .toPromise()
      .then(data => {
        this.appConfig = data;
      });
  }

  getApiUrl() {
    return this.appConfig.api;
  }


}

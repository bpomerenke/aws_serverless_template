import { TestBed } from '@angular/core/testing';

import { ApiService } from './api.service';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { AppConfigService } from '../app-config.service';
import { VersionInfo } from '../models/version';
import { Message } from '../models/message';

describe('ApiService', () => {
  beforeEach(() => {
    const httpClient = jasmine.createSpyObj<HttpClient>(['get', 'post']);
    const appConfigService = jasmine.createSpyObj<AppConfigService>(['getApiUrl']);
    TestBed.configureTestingModule({
      providers: [
        { provide: HttpClient, useValue: httpClient },
        { provide: AppConfigService, useValue: appConfigService }
      ]
    })
  });

  describe('getVersion', () => {
    it('should get version from the api', (done) => {
      const service: ApiService = TestBed.get(ApiService);
      const httpClient = TestBed.get(HttpClient);
      const appConfig = TestBed.get(AppConfigService);

      const expectedBaseUrl = 'url goes here';
      const expectedVersion = { version: 'foo' } as VersionInfo;
      httpClient.get.and.returnValue(of(expectedVersion));
      appConfig.getApiUrl.and.returnValue(expectedBaseUrl);

      service.getVersion().subscribe(data => {
        const recentCall = httpClient.get.calls.argsFor(0);
        expect(data).toEqual(expectedVersion);
        expect(recentCall[0]).toEqual(`${expectedBaseUrl}/version`);

        done();
      });
    });
  });

  describe('getMessages', () => {
    it('should get messages from the api', (done) => {
      const service: ApiService = TestBed.get(ApiService);
      const httpClient = TestBed.get(HttpClient);
      const appConfig = TestBed.get(AppConfigService);

      const expectedBaseUrl = 'url goes here';
      const expectedMessages = [{ clientId: 'foo' } as Message];
      httpClient.get.and.returnValue(of(expectedMessages));
      appConfig.getApiUrl.and.returnValue(expectedBaseUrl);

      service.getMessages().subscribe(data => {
        const recentCall = httpClient.get.calls.argsFor(0);
        expect(data).toEqual(expectedMessages);
        expect(recentCall[0]).toEqual(`${expectedBaseUrl}/messages`);

        done();
      });
    });
  });

  describe('sendMessage', () => {
    it('sends message to the api', (done) => {
      const service: ApiService = TestBed.get(ApiService);
      const httpClient = TestBed.get(HttpClient);
      const appConfig = TestBed.get(AppConfigService);

      const expectedBaseUrl = 'url goes here';
      httpClient.post.and.returnValue(of({}));
      appConfig.getApiUrl.and.returnValue(expectedBaseUrl);

      const message = 'hello';
      service.sendMessage(message).subscribe(() => {
        const recentCall = httpClient.post.calls.argsFor(0);
        expect(recentCall[0]).toEqual(`${expectedBaseUrl}/messages`);
        expect(recentCall[1].msgText).toEqual(message);

        done();
      });
    });
  });

  

});

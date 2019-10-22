import { TestBed } from '@angular/core/testing';

import { WebsocketService } from './websocket.service';
import { AppConfigService } from '../app-config.service';

describe('WebsocketService', () => {
  beforeEach(() => {
    const appConfigService = jasmine.createSpyObj<AppConfigService>(['getWsUrl']);
    TestBed.configureTestingModule({
      providers: [
        { provide: AppConfigService, useValue: appConfigService }
      ]
    })
  });

  it('should be created', () => {
    const service: WebsocketService = TestBed.get(WebsocketService);
    expect(service).toBeTruthy();
  });
});

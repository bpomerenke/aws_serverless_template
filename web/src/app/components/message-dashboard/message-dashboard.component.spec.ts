import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageDashboardComponent } from './message-dashboard.component';
import { ApiService } from 'src/app/services/api.service';
import { Message } from 'src/app/models/message';
import { of, throwError, Subscriber, observable, Observable } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { WebsocketService } from 'src/app/services/websocket.service';
import { FormsModule } from '@angular/forms';

describe('MessageDashboardComponent', () => {
  let component: MessageDashboardComponent;
  let fixture: ComponentFixture<MessageDashboardComponent>;

  beforeEach(async(() => {
    const apiService = jasmine.createSpyObj<ApiService>(['getMessages', 'sendMessage']);
    const wsService = jasmine.createSpyObj<WebsocketService>(['connect']);
    TestBed.configureTestingModule({
      imports: [FormsModule],
      declarations: [ MessageDashboardComponent ],
      providers: [
        { provide: ApiService, useValue: apiService },
        { provide: WebsocketService, useValue: wsService }
      ],
      schemas: [NO_ERRORS_SCHEMA]

    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MessageDashboardComponent);
    component = fixture.componentInstance;
  });

  it('should have messages when api call succeeds', () => {
    const apiService = TestBed.get(ApiService);
    const wsService = TestBed.get(WebsocketService);

    const messages = [{ clientId: 'foo'} as Message];
    apiService.getMessages.and.returnValue(of(messages));
    wsService.connect.and.returnValue(of());

    fixture.detectChanges();

    expect(component.messages).toEqual(messages);
    expect(component.messageError).toBeUndefined();
    expect(wsService.connect).toHaveBeenCalled();
  });
  it('should have error when api call fails', () => {    
    const apiService = TestBed.get(ApiService);

    const expectedError = 'problem fetching';
    apiService.getMessages.and.returnValue(throwError(expectedError));

    fixture.detectChanges();

    expect(component.messages).toBeUndefined();
    expect(component.messageError).toEqual(expectedError);
  });
  it('should append new messages when subscription returns', () => {
    const apiService = TestBed.get(ApiService);
    const wsService = TestBed.get(WebsocketService);

    const messages = [{ clientId: 'foo'} as Message];
    let testSubscriber: Subscriber<any>;
    apiService.getMessages.and.returnValue(of(messages));
    wsService.connect.and.returnValue(new Observable<any>(s => testSubscriber = s));

    fixture.detectChanges();

    expect(component.messages).toEqual(messages);
    expect(wsService.connect).toHaveBeenCalled();

    const newMessage = { clientId: 'bar'} as Message;
    testSubscriber.next(newMessage);

    expect(component.messages.length).toEqual(2);
    expect(component.messages[0]).toEqual(newMessage);
  });

  describe('sendMessage', ()=>{
    it('should send message', () => {
      const apiService = TestBed.get(ApiService);

      let testSubscriber: Subscriber<any>;
      apiService.sendMessage.and.returnValue(new Observable<any>(s=>testSubscriber = s))
      const message = 'hello there';
      component.curMessage = message;
      component.sendMessage();
      
      expect(component.sendingMessage).toEqual(true);
      expect(apiService.sendMessage).toHaveBeenCalledWith(message);

      testSubscriber.next({});

      expect(component.sendingMessage).toEqual(false);
      expect(component.curMessage).toEqual('');
    });
  });
});

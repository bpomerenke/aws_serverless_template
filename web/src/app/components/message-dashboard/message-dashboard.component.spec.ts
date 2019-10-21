import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageDashboardComponent } from './message-dashboard.component';
import { ApiService } from 'src/app/services/api.service';
import { Message } from 'src/app/models/message';
import { of, throwError } from 'rxjs';

describe('MessageDashboardComponent', () => {
  let component: MessageDashboardComponent;
  let fixture: ComponentFixture<MessageDashboardComponent>;

  beforeEach(async(() => {
    const apiService = jasmine.createSpyObj<ApiService>(['getMessages']);
    TestBed.configureTestingModule({
      declarations: [ MessageDashboardComponent ],
      providers: [
        { provide: ApiService, useValue: apiService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MessageDashboardComponent);
    component = fixture.componentInstance;
  });

  it('should have messages when api call succeeds', () => {
    const apiService = TestBed.get(ApiService);

    const messages = [{ clientId: 'foo'} as Message];
    apiService.getMessages.and.returnValue(of(messages));

    fixture.detectChanges();

    expect(component.messages).toEqual(messages);
    expect(component.messageError).toBeUndefined();
  });
  it('should have error when api call fails', () => {    
    const apiService = TestBed.get(ApiService);

    const expectedError = 'problem fetching';
    apiService.getMessages.and.returnValue(throwError(expectedError));

    fixture.detectChanges();

    expect(component.messages).toBeUndefined();
    expect(component.messageError).toEqual(expectedError);
  });
});

import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { ApiService } from './services/api.service';
import { of } from 'rxjs';
import { VersionInfo } from './models/version';

describe('AppComponent', () => {
  beforeEach(async(() => {
    const apiService = jasmine.createSpyObj<ApiService>(['getVersion']);
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent
      ],
      providers: [
        { provide: ApiService, useValue: apiService }
      ]
    }).compileComponents();
  }));

  it('should fetch version', () => {
    const apiService = TestBed.get(ApiService);
    const fixture = TestBed.createComponent(AppComponent);
    const component = fixture.componentInstance;

    const expectedVersion = '0.4';
    apiService.getVersion.and.returnValue(of({version: expectedVersion} as VersionInfo));

    fixture.detectChanges();
    
    expect(component.version).toEqual(expectedVersion);
  });
});

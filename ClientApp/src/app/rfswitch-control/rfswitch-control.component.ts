import { Component,  OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subscription } from 'rxjs/Rx';
import { delay } from 'q';

import { RFSwitchService, RFSwitchDevice, MCU, RfCode } from '../rfswitchcontrol/rfswitch.service';

@Component({
  selector: 'app-rfswitch-control',
  templateUrl: './rfswitch-control.component.html'
})
export class RFSwitchControlComponent implements OnInit, OnDestroy {
  public lastUpdate = 'Unavailable'
  private sub: Subscription; 
  //public switchList: RFSwitchStatus[];
  public rfswitchService: RFSwitchService;

  constructor(_rfswitchService: RFSwitchService ) {
    this.rfswitchService = _rfswitchService;
    console.log('RFSwitchControlComponent.constructor(): Entered');
  }

  ngOnInit() {
    var switches = this.rfswitchService.callAPI('');
    
    //let cnt: number = 0;

    //while (!switches && cnt++ < 3) {
    //  delay(1000);
    //  switches = this.rfswitchService.callAPI('');
    //}

    //if (switches) {
    //  this.switchList = switches;
    //  this.lastUpdate = this.rfswitchService.lastUpdate;
    //}

    var ticker = Observable.timer(5000, 60000);
    this.sub = ticker.subscribe(x => this.reload());
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  reload() {
    this.callWebApi('');
  }

  handleToggle(event: Event) {      
    let str: string = event.srcElement.id.substring(6);
     
    let sw1 = this.rfswitchService.detections.find(s => s.localDeviceId == parseInt(str));
    let swState = sw1.swState == 0 ? 1 : 0;

    var upd = this.rfswitchService.setSwitch(sw1.localDeviceId, swState);
    if (upd != null) {
      sw1.lastOffCode = upd.lastOffCode;
      sw1.lastOnCode = upd.lastOnCode;
      sw1.swState = upd.swState;
    }
  }

  callWebApi(apiCall: string) {
    //console.log('callWebApi() Entered. apiCall: ' + apiCall);
    var list = this.rfswitchService.callAPI(apiCall);
    //this.lastUpdate = this.rfswitchService.lastUpdate;
  }  
}

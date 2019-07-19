import { Component, OnInit} from '@angular/core';
import { PlayersService } from '../../api/players.service';
import { Player } from '../../model/player';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  player?: Player;
  logged = true;
  hasProfile = true;
  userName: string = this.getValueFromIdToken('name');

  constructor(private playersService: PlayersService) { }

  ngOnInit() {
    this.playersService.playersGetPlayerByUserId().subscribe(
      b => {
          this.player = b;
          if (b.gameSession == null)
          {
            this.hasProfile = false;
          }
      },
      (err: HttpErrorResponse) => {
        return console.log(err.error);
      }
    );
    }

  getValueFromIdToken(claim: string) {
    const jwt = sessionStorage.getItem('id_token');
    if ( jwt == null) {
      this.logged = false;
      return null;
    }
}
}

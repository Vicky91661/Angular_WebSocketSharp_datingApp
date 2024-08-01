import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private webSocket: WebSocket;
  public messages$ = new BehaviorSubject<any[]>([]);
  public connectedUsers$ = new BehaviorSubject<string[]>([]);
  public messages: any[] = [];
  public connectedUsers: string[] = [];

  constructor() {
    this.webSocket = new WebSocket('ws://localhost:6969/chat');
    this.webSocket.onopen = () => {
      console.log('WebSocket connection opened.');
    };

    this.webSocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      switch (data.Type) {
        case 'ReceivedMessage':
          this.messages = [...this.messages, { user: data.User, message: data.Message, messageTime: data.Time }];
          this.messages$.next(this.messages);
          break;
        case 'ConnectedUser':
          this.connectedUsers = data.Users;
          this.connectedUsers$.next(this.connectedUsers);
          break;
      }
    };

    this.webSocket.onerror = (error) => {
      console.error('WebSocket error:', error);
    };

    this.webSocket.onclose = () => {
      console.log('WebSocket connection closed.');
    };
  }

  // Join the room
  public joinRoom(user: string, room: string) {
    const message = JSON.stringify({ Type: 'join', User: user, Room: room });
    this.webSocket.send(message);
  }

  // Send Messages
  public sendMessage(message: string) {
    const user = sessionStorage.getItem('user');
    const roomMessage = JSON.stringify({ Type: 'message', User: user, Message: message });
    this.webSocket.send(roomMessage);
  }

  // leave
  public leaveChat() {
    this.webSocket.close();
  }
}

import React, { Component } from 'react';
import WebSocketExample from "./WebSocketExample";

export class Home extends Component {
  static displayName = Home.name;

  //         <WebSocketExample/>
  render () {
    return (
      <div>
        <h1>Welcome to the great ecommerce store</h1>
        {/*<WebSocketExample/>*/}
      </div>
    );
  }
}

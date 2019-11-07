import React from "react";
import {render} from "react-dom";
import ReactDOM from "react-dom";
import axios from "axios";

export class SignupForm extends React.Component {
  constructor() {
    super();
    this.state = {
      username: "",
      password: "",
      passwordC: "",
      email: ""
    }
  }

  onChange(e) {
    //determine which text input is changing
    if(e.target.name === "user"){
      if(e.target.value !== this.state.username){
        this.setState({
          username: e.target.value
        });
      }
    }
    else if(e.target.name === "pass"){
      if(e.target.value !== this.state.password){
        this.setState({
          password: e.target.value
        });
      }
    }
    else if(e.target.name === "passC"){
      if(e.target.value !== this.state.passwordC){
        this.setState({
          passwordC: e.target.value
        });
      }
    }
    else {
      if(e.target.value !== this.state.email){
        this.setState({
          email: e.target.value
        });
      }
    }
  }

  handlePost(e) {
    //verify that the account information is legal
    if(this.state.password.length < 8){
      window.alert("Password must be 8 characters");
    }
    else if(this.state.username.length < 8){
      window.alert("Password must be 8 characters");
    }
    else if(this.state.email.length < 6){
      window.alert("Email muest exceed 6 characters");
    }
    else if(this.state.password !== this.state.passwordC){
      window.alert("Passwords must match.");
    }
    else if(this.state.email.includes("@") === false){
      window.alert("Must be a valid email.");
    }
    else{
      axios.post('/signup', {
        username: this.state.username,
        password: this.state.password,
        email: this.state.email
      }).then(
        response => {
          window.alert("Should have worked.");
        },
        error => {
          window.alert("An error occured. Please try again later.");
        }
      );
    }
  }

  render () {
    return (
      <div className="container">
        <div className="row">
          <div className="col">
            <input type="text" name="email" placeholder="Email" value={this.state.email} onChange={this.onChange.bind(this)}></input>
            <input type="text" name="user" placeholder="Username" value={this.state.username} onChange={this.onChange.bind(this)}></input>
            <input type="password" name="pass" placeholder="Password" value={this.state.password} onChange={this.onChange.bind(this)}></input>
            <input type="password" name="passC" placeholder="Confirm Password" value={this.state.passwordC} onChange={this.onChange.bind(this)}></input>
            <button onClick={this.handlePost.bind(this)}>Create Account</button>
          </div>
        </div>
      </div>
    );
  }
}

ReactDOM.render(<SignupForm/>, window.document.getElementById("signup"));

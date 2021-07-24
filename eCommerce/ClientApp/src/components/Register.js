import React, { Component } from 'react';
import { AuthApi } from "../Api/AuthApi"
import "./Register.css"
import {withRouter} from "react-router-dom";

class Register extends Component {
    static displayName = Register.name;

    constructor(props) {
        super(props);
        this.state = { 
            registrationError: undefined,
            username: undefined,
            password: undefined,
            email: undefined,
            name: undefined,
            address: undefined,
            birthday: undefined
        };
        this.authApi = new AuthApi();
        
        this.handleInputChange = this.handleInputChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) history.push(path);
    }
    
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }
    
    async handleSubmit(event){
        event.preventDefault();
        const {username, password, email, name, address, birthday} = this.state;
        const registerRedirectAndRes = await this.authApi.Register(username, password, email, name, address, birthday);
        if(registerRedirectAndRes) {
            const registerRes = registerRedirectAndRes.data;

            if (registerRes && registerRes.isSuccess) {
                this.redirectToHome(registerRedirectAndRes.redirect)
            } else {
                this.setState({
                    registrationError: registerRes.error
                })
            }
        } else {
            this.setState({
                registrationError: "Error"
            })
        }
    }

    componentDidMount() {
        //this.populateWeatherData();
    }
    
    render() {
        return (
            <main class="RegisterMain">
                <div class="RegisterWindow">
                    <div class="CenterItemContainer">
                        <h3>Register</h3>
                    </div>
                    <form class="RegisterForm" onSubmit={this.handleSubmit}>
                        {this.state.registrationError ? <div class="CenterItemContainer"><label>{this.state.registrationError}</label></div> : null}
                        <input type="text" name="username" value={this.state.username} onChange={this.handleInputChange} placeholder="Username" required/>
                        <input type="password" name="password" value={this.state.password} onChange={this.handleInputChange} placeholder="Password" required/>
                        <input type="email" name="email" value={this.state.email} onChange={this.handleInputChange} placeholder="email@email.com" required/>
                        <input type="text" name="name" value={this.state.name} onChange={this.handleInputChange} placeholder="Your name" required/>
                        <input type="text" name="address" value={this.state.address} onChange={this.handleInputChange} placeholder="Your address" required/>
                        <input type="date" name="birthday" value={this.state.birthday} onChange={this.handleInputChange} required/>
                        <div className="CenterItemContainer">
                            <input class="action" type="submit" value="Register"/>
                        </div>
                    </form>
                </div>
            </main>
        );
    }
}

export default withRouter(Register);

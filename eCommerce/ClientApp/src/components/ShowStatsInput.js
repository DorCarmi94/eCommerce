import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";

export class ShowStatsInput extends Component {
    static displayName = ShowStatsInput.name;

    constructor(props) {
        super(props)
        this.state = {
            date:'',
            submitted: false
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    async handleSubmit(event){
        this.setState({
            submitted:true
        })

    }
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }

    render () {
        if (this.state.submitted) {
            return <Redirect to={`showStats/${this.state.date}`}/>
        } else {
            return (
                <main className="RegisterMain">
                    <div className="RegisterWindow">
                        <div className="CenterItemContainer">
                            <h3>Show Stats - Enter Date In Format mm-dd-yyyy</h3>
                        </div>
                        <form className="RegisterForm" onSubmit={this.handleSubmit}>
                            <input type="text" name="date" value={this.state.date}
                                   placeholder={'Enter Date In Format mm-dd-yyyy'} onChange={this.handleInputChange} required/>
                            <div className="CenterItemContainer">
                                <input className="action" type="submit" value="submit"/>
                            </div>
                        </form>
                    </div>
                </main>
            );
        }
    }
}
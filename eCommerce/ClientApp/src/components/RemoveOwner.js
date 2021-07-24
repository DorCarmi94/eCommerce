import React, {Component} from "react";
import {Form,Button} from 'react-bootstrap'
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";

export class RemoveOwner extends Component {
    static displayName = RemoveOwner.name;

    constructor(props) {
        super(props)
        this.state = {
            ownerId:'',
            submitted: false
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    async handleSubmit(event){
        const {ownerId} = this.state
        const {storeId} = this.props
        event.preventDefault();
        const res = await this.storeApi.removeOwner(storeId,ownerId)
        if(res && res.isSuccess) {
            this.setState({
                submitted: true
            })
            alert(`${ownerId} is no longer owner of the store: ${storeId}`)
        }
        else{
            if(res) {
                alert(`remove owner failed because- ${res.error}`)
            }
        }

    }
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }

    render () {
        if (this.state.submitted) {
            return <Redirect exact to={`/managePermissions/${this.props.storeId}`}/>
        } else {
            return (
                <main className="RegisterMain">
                    <div className="RegisterWindow">
                        <div className="CenterItemContainer">
                            <h3>Remove Owner</h3>
                        </div>
                        <form className="RegisterForm" onSubmit={this.handleSubmit}>
                            <input type="text" name="ownerId" value={this.state.ownerId}
                                   placeholder={'Enter Owner Id To Remove'} onChange={this.handleInputChange} required/>
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
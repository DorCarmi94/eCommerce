import React, {Component} from "react";
import {Form,Button} from 'react-bootstrap'
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";

export class RemoveManager extends Component {
    static displayName = RemoveManager.name;

    constructor(props) {
        super(props)
        this.state = {
            managerId:'',
            submitted: false
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    async handleSubmit(event){
        const {managerId} = this.state
        const {storeId} = this.props
        event.preventDefault();
        const res = await this.storeApi.removeManager(storeId,managerId)
        if(res && res.isSuccess) {
            this.setState({
                submitted: true
            })
            alert(`${managerId} is no longer manager of the store: ${storeId}`)
        }
        else{
            if(res) {
                alert(`remove manager failed because- ${res.error}`)
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
                            <h3>Remove Manager</h3>
                        </div>
                        <form className="RegisterForm" onSubmit={this.handleSubmit}>
                            <input type="text" name="managerId" value={this.state.managerId}
                                   placeholder={'Enter Manager Id To Remove'} onChange={this.handleInputChange} required/>
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
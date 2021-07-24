import React, {Component} from "react";
import {Form,Button} from 'react-bootstrap'
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect} from "react-router-dom";

export class AdminPurchaseHistory extends Component {
    static displayName = AdminPurchaseHistory.name;

    constructor(props) {
        super(props)
        const {term} = props
        this.state = {
            searchTerm:'',
            submitted: false,
            term:term
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    async handleSubmit(event){
        event.preventDefault();
            this.setState({
                submitted: true
            })
        }
        componentDidUpdate(prevProps, prevState, undefined) {
            if (prevProps.term !== this.props.term) {
                this.setState({
                    term:this.props.term
                })
            }

        }

    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }

    render () {
        if (this.state.submitted && this.state.term === 'Store') {
            return <Redirect exact to={`/purchaseHistory/${this.state.searchTerm}/ /true`}/>
        }
        else if (this.state.submitted && this.state.term === 'User') {
            return <Redirect exact to={`/purchaseHistory/ /${this.state.searchTerm}/true`}/>
        }else {
            return (
                <main className="RegisterMain">
                    <div className="RegisterWindow">
                        <div className="CenterItemContainer">
                            <h3>{`Search Purchase History For ${this.state.term}`}</h3>
                        </div>
                        <form className="RegisterForm" onSubmit={this.handleSubmit}>
                            <input type="text" name="searchTerm" value={this.state.searchTerm}
                                   placeholder={`Enter ${this.state.term} Id`} onChange={this.handleInputChange} required/>
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

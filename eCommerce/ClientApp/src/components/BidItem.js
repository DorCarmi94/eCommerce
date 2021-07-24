import React, { Component } from 'react';
import {StoreApi} from '../Api/StoreApi'
import "./Register.css"
import {Redirect, withRouter} from "react-router-dom";

class BidItem extends Component {
    static displayName = BidItem.name;

    constructor(props) {
        super(props);
        this.state = {
            amount:'',
            newPrice:'',
            submitted:false
        };

        this.handleInputChange = this.handleInputChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.storeApi = new StoreApi();

    }
    

    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }

    async handleSubmit(event){
        event.preventDefault();
        const {amount,newPrice} = this.state
        const {itemId, storeId} = this.props
        const res = await this.storeApi.askToBidOnItem(storeId, itemId, amount, newPrice);
        if(res && res.isSuccess) {
            alert('bit item succeed')
            this.setState({
                submitted:true
            })
        }
        else{
            if(res) {
                alert(`bid item failed because- ${res.error}`)
            }
        }
    }

    componentDidMount() {
        //this.populateWeatherData();
    }

    render() {
        const {submitted,amount,newPrice} = this.state
        const {storeId,itemId} = this.props
        if (submitted) {
            return <Redirect exact to={`/store/${storeId}`}/>
        } 
        else {
            return (
                <main class="RegisterMain">
                    <div class="RegisterWindow">
                        <div class="CenterItemContainer">
                            <h3>{`Ask To Bid Item: ${itemId} In Store: ${storeId}`}</h3>
                        </div>
                        <form class="RegisterForm" onSubmit={this.handleSubmit}>
                            <input type="number" name="amount" value={amount}
                                   onChange={this.handleInputChange} placeholder="amount" required/>
                            <input type="number" name="newPrice" value={newPrice}
                                   onChange={this.handleInputChange} placeholder="newPrice" required/>
                            <div className="CenterItemContainer">
                                <input class="action" type="submit" value="Bid Item"/>
                            </div>
                        </form>
                    </div>
                </main>
            );
        }
    }
}

export default withRouter(BidItem);

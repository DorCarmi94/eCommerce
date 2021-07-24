import React, {ChangeEvent, Component} from "react";
import "../StyleForm.css"
import {Redirect, withRouter} from "react-router-dom";
import {CartApi} from "../../Api/CartApi";

interface PurchaseCartState {
    idNumber: string, 
    creditCardNumber: number | undefined,
    creditCardExpirationDate: string, 
    threeDigitsOnBackOfCard: number | undefined,
    fullAddress: string,
    message: string,
    submitted: boolean
}

interface PurchaseCartProps {
    username: string
}

export class PurchaseCart extends Component<PurchaseCartProps, PurchaseCartState> {
    static displayName = PurchaseCart.name;
    private cartApi: CartApi;

    constructor(props: PurchaseCartProps) {
        super(props)
        this.state = {
            idNumber: "",
            creditCardNumber: undefined,
            creditCardExpirationDate: "",
            threeDigitsOnBackOfCard: undefined,
            fullAddress: "",
            message: "",
            submitted: false
        }
        this.cartApi = new CartApi();
        
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChangeString = this.handleInputChangeString.bind(this);
        this.handleInputChangeNumber = this.handleInputChangeNumber.bind(this);

    }

    async handleSubmit(event: React.FormEvent){
        event.preventDefault();
        const {idNumber, creditCardNumber,
            creditCardExpirationDate, threeDigitsOnBackOfCard, fullAddress} = this.state

        if(creditCardNumber && threeDigitsOnBackOfCard) {
            this.setState({
                message: "Purchasing.."
            })
            const purchaseRes = await this.cartApi.PurchasePrice(
                this.props.username, idNumber, creditCardNumber.toString(),
                creditCardExpirationDate, threeDigitsOnBackOfCard.toString(), fullAddress
            )
            
            console.log(purchaseRes);
            if(!purchaseRes || purchaseRes.isFailure){
                this.setState({
                    message: `Error ${purchaseRes?.error}`
                });
                return;
            }
        
            this.setState({
                message: "Successful purchase"
            })
            
            setTimeout(() => this.setState({submitted: true}), 2000);
        }
    }
    
    handleInputChangeString(event: ChangeEvent<HTMLInputElement>){
        const target = event.target;
        // @ts-ignore
        this.setState({
            [target.name]: target.value
        });
    }

    handleInputChangeNumber(event: ChangeEvent<HTMLInputElement>){
        const target = event.target;
        // @ts-ignore
        this.setState({
            [target.name]: target.value
        });
    }

    render () {
        const { submitted, message } = this.state
        return (
            submitted ?
                <Redirect exact to="/cart"/> :
                <main className="FormMain">
                    <div className="FormWindow">
                        <div className="CenterItemContainer">
                            <h3>Purchase cart</h3>
                        </div>
                        {message.length > 0 ?
                            <div className="CenterItemContainer">
                                <h5 className="cr_purple">{message}</h5>
                            </div> :
                            null}
                        <form className="Form" onSubmit={this.handleSubmit}>
                            <input type="text" name="idNumber" value={this.state.idNumber}
                                   placeholder={'ID number'} onChange={this.handleInputChangeString} required/>
                            <input type="number" name="creditCardNumber" value={this.state.creditCardNumber} onChange={this.handleInputChangeNumber}
                                   placeholder={'credit card number'} required/>
                            <input type="date" name="creditCardExpirationDate" value={this.state.creditCardExpirationDate} onChange={this.handleInputChangeString}
                                   placeholder={'Credit card expiration date'} required/>
                            <input type="number" name="threeDigitsOnBackOfCard" value={this.state.threeDigitsOnBackOfCard} onChange={this.handleInputChangeNumber}
                                   placeholder={'Three digits on back of card'} minLength={3} maxLength={3} required/>
                            <input type="text" name="fullAddress" value={this.state.fullAddress} onChange={this.handleInputChangeString}
                                   placeholder={'Delivery address'} required/>
                            <div className="CenterItemContainer">
                                <input className="action" type="submit" value="submit"/>
                            </div>
                        </form>
                    </div>
                </main>
        );
    }
}
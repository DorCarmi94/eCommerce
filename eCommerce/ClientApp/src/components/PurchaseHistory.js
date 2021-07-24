import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StoreApi} from "../Api/StoreApi";
import {UserApi} from "../Api/UserApi";
import {Link} from "react-router-dom";
import {Item} from "../Data/Item";
import {StorePermission} from '../Data/StorePermission'
import {NavLink} from "reactstrap";

export default class PurchaseHistory extends Component {
    static displayName = PurchaseHistory.name;

    constructor(props) {
        super(props)
        const {storeId,userId,isAdmin} = props
        this.state = {
            storeId: storeId,
            userId:userId,
            isAdmin:isAdmin,
            historyDetails: [],
        }
        this.storeApi = new StoreApi();
        this.userApi = new UserApi();
    }

    async getHistory(){
        const {storeId,userId,isAdmin} = this.state
        console.log('storeId ' + this.state.storeId)
        console.log('userId ' + this.state.userId)
        console.log('isAdmin ' + this.state.isAdmin)
        let res = undefined
        if(isAdmin && storeId && userId === ' '){
            res = await this.storeApi.adminGetStorePurchaseHistory(storeId)
        }
        else if(storeId && !userId && !isAdmin) {
            res = await this.storeApi.getPurchaseHistory(this.state.storeId)
        }
        else if (storeId === ' ' && userId !== ' ' && isAdmin){
            res = await this.userApi.adminGetPurchaseHistory(this.state.userId)
        }
        else{
            res = await this.userApi.getPurchaseHistory()
        }
        if (res && res.isSuccess) {
            console.log(res.value)
            this.setState({
                historyDetails: res.value.records
            })
        }
    }

    async componentDidMount() {
        await this.getHistory();
    }

    async componentDidUpdate(prevProps, prevState, undefined) {
        // if (prevProps.storeId !== this.props.storeId) {
        //     console.log(`update `);
        //     console.log(this.props);
        //     console.log(prevProps);
        //     await this.setState({
        //         storeId: this.props.storeId
        //     })
        //     await this.getItems();
        // }
    }

    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) {
            alert('succed')
            history.push(path);
        }
    }

    removeItem = async (storeId,itemId) =>
    {
        const res = await this.storeApi.removeItem(storeId,itemId)

        if(res && res.isSuccess) {
            alert('edit item succeed')
            // this.props.addStoreToState(storeId);
            this.redirectToHome('/')
        }
        else{
            if(res) {
                alert(`edit item failed because- ${res.error}`)
            }
        }

    }


    
    

    render() {
        const { storeId, permissions} = this.state
        return (
            // <div>Store History</div>)
            <div>
                <Table striped bordered hover>
                    <thead>
                    <tr>
                        <th>#</th>
                        <th>User Name</th>
                        <th>Store ID</th>
                        <th>Item</th>
                        <th>Amount</th>
                        <th>Total Price</th>
                        <th>Date</th>
                    </tr>
                    </thead>
                    <tbody>
                    
                    {
                        this.state.historyDetails.map((detail, index) => {
                            return (
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{detail.username}</td>
                                    <td>{detail.storeId}</td>
                                    <td>{detail.basket.items.map((item) => <div>{item.itemName}</div>)}</td>
                                    <td>{detail.basket.items.map((item) => <div>{item.amount}</div>)}</td>
                                    <td>{detail.basket.totalPrice}</td>
                                    <td>{detail.purchaseTime}</td>

                                </tr>
                            )
                        })
                    }
                    </tbody>
                </Table>
            </div>
        );
    }

}

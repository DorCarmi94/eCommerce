import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {BidStateNames} from "../Data/BidInfo";
import {StoreApi} from "../Api/StoreApi";
import {makeDiscountNodeLeaf} from "../Data/StorePolicies/DiscountInfoTree";

export default class ShowBids extends Component {
    static displayName = ShowBids.name;

    constructor(props) {
        super(props)
        this.state = {
            bids:[],
            actionOccured:true
        }
        this.storeApi = new StoreApi()
    }
    
    async fetchBids(){
        const {storeId} = this.props
        const fetchedBids = await this.storeApi.getAllBidWaiting(storeId)
        console.log("stats")
        console.log(fetchedBids)
        if (fetchedBids && fetchedBids.isSuccess) {
            this.setState({
                bids: fetchedBids.value
            })

        }
    }

    async componentDidMount() {
        await this.fetchBids()
    }

    async componentDidUpdate(prevProps, prevState, snapshot){
        if(this.state.actionOccured !== prevState.actionOccured ) {
            await this.fetchBids()
        }
    }
    
    async approveOrDisapproveBid (event,bidId,shouldApprove){
        const {storeId} = this.props
        const res = await this.storeApi.approveOrDisapproveBid(storeId,bidId,shouldApprove)
        if(res && res.isSuccess){
            alert(`bid:${bidId} ${shouldApprove ? 'approved' : 'disapproved'} successfully`)
            this.setState({
                actionOccured:!this.state.actionOccured
            })
        }
        else{
            if(res){
                alert(`operation failed because ${res.error}`)
            }
        }
    }

  
    render() {
        const {bids} = this.state
        const {storeId} = this.props
        return (
                <div style={{marginTop: "10px"}}>
                    <h3>Bids For Store {storeId}</h3>
                    <Table striped bordered hover>
                        <thead>
                        <tr>
                            <th>Buyer ID</th>
                            <th>Item ID</th>
                            <th>Bid Amount</th>
                            <th>Bid Price</th>
                            <th>Bid State</th>

                        </tr>
                        </thead>
                        <tbody>

                        {
                            bids.map((bid) => {
                                console.log(bid.bidID)
                                return (
                                    <tr>
                                        <td>{bid.buyerID}</td>
                                        <td>{bid.itemID}</td>
                                        <td>{bid.amount}</td>
                                        <td>{bid.price}</td>
                                        <td>{BidStateNames[bid.state]}</td>
                                        <td><button className = "cr_green" onClick={(event) => this.approveOrDisapproveBid(event,bid.bidID,true)}>Aprrove</button>
                                            <button className = "cr_red" onClick={(event) => this.approveOrDisapproveBid(event,bid.bidID,false)}>Disaprrove</button></td>
                                    </tr>
                                )
                            })
                        }
                        {/*{*/}
                        {/*    stats.map((stat) => {*/}
                        {/*        return (<tr>*/}
                        {/*            <td>{stat.Item1}</td>*/}
                        {/*            <td>{stat.Item2}</td>*/}
                        {/*        </tr>)*/}
                        {/*    })*/}
                        {/*}*/}
                        </tbody>
                    </Table>
                </div> 
        );
    }
}

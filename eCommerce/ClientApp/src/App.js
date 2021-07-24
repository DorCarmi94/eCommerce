import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Login } from "./components/Login";

import Store from './components/Store'
import { OpenStore } from "./components/OpenStore";
import { Cart } from "./components/Cart"
import Register from "./components/Register";
import './custom.css'
import AddItem from './components/AddItem'
import EditItem from './components/EditItem'
import PurchaseHistory from './components/PurchaseHistory'
import ManagePermissions from './components/ManagePermissions'

import {BrowserRouter, Link, useHistory} from "react-router-dom";
import {UserApi} from "./Api/UserApi";
import {ItemSearchDisplay} from "./components/ItemsSearchDisplay";
import {SearchComponent} from "./components/SearchComponent";
import {PurchaseCart} from "./components/Cart/PurchaseCart";
import {HttpTransportType, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {StoreApi} from "./Api/StoreApi";
import {AppointManager} from "./components/AppointManager";

import {AdminPurchaseHistory} from "./components/AdminPurchaseHistory";
import {UserRole} from "./Data/UserRole";
import {makeRuleInfo, makeRuleNodeComposite, makeRuleNodeLeaf, RuleType} from "./Data/StorePolicies/RuleInfo";
import {Comperators} from "./Data/StorePolicies/Comperators";
import {Combinations} from "./Data/StorePolicies/Combinations";
import {makeDiscountCompositeNode, makeDiscountNodeLeaf} from "./Data/StorePolicies/DiscountInfoTree";
import AddRule from "./components/AddRule";
import {AppointOwner} from "./components/AppointOwner";
import {ShowStatsInput} from "./components/ShowStatsInput";
import ShowStatsOutput from "./components/ShowStatsOutput";
import AddDiscount from "./components/AddDiscount";
import {RemoveManager} from "./components/RemoveManager";
import {RemoveOwner} from "./components/RemoveOwner";
import BidItem from "./components/BidItem";
import ShowBids from "./components/ShowBids";

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props)
        this.state = {
            isLoggedIn: false,
            ownedStoreList:[],
            userName:'',
            role: undefined,

            messages: [],
            webSocketConnection: undefined
        }
        this.userApi = new UserApi();
        this.storeApi = new StoreApi();

        this.addOwnedStoreHandler = this.addOwnedStoreHandler.bind(this);
        this.updateLoginHandler = this.updateLoginHandler.bind(this);
        this.updateLogoutHandler = this.updateLogoutHandler.bind(this);

    }

    addOwnedStoreHandler(store){
        this.setState({
            ownedStoreList:[...this.state.ownedStoreList, store]
        });
    }

    async updateLoginHandler(username, role){
        this.setState({
            isLoggedIn: true,
            userName: username,
            role: role,
        })
    }

    async updateLogoutHandler(){
        const userBasicInfo = await this.userApi.getUserBasicInfo();
        await this.state.webSocketConnection.stop();
        this.setState({
            isLoggedIn: userBasicInfo.isLoggedIn,
            userName: userBasicInfo.username,
            role: userBasicInfo.userRole,
            ownedStoreList: [],
            managedStoreList:[],
            webSocketConnection: undefined
        })
    }

    async connectToWebSocket(){
        const socketConnection = new HubConnectionBuilder()
            .configureLogging(LogLevel.Debug)
            .withUrl("https://localhost:5001/messageHub", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .build();

        //console.log("socketConnection")

        await socketConnection.start();
        if(socketConnection) {
            //console.log("socketConnection on")

            socketConnection.on("message", message => {
                /*console.log("new publisher message:")
                console.log(message)*/
                alert(`New message: ${message.message}`)
                this.setState({
                    messages: [...this.state.messages, message]
                });
            });
        }

        this.setState({
            messages: [],
            webSocketConnection: socketConnection
        });
    }

    redirectToHome = (path) => {
        alert(path)
        const { history } = this.props;
        if(history) {
            alert('succed')
            history.push(path);
        }
    }

    async policyExample(){
        console.log("Policy example: ==========")
        const ruleInfo1 = makeRuleInfo(RuleType.Amount, "10", "3", Comperators.EQUALS);
        const ruleInfo2 = makeRuleInfo(RuleType.Category, "watermelon", "1", Comperators.BIGGER);

        const ruleNodeLeaf1 = makeRuleNodeLeaf(ruleInfo1);
        const ruleNodeLeaf2 = makeRuleNodeLeaf(ruleInfo2);
        console.log(await this.storeApi.addRuleToStorePolicy("a", ruleNodeLeaf1));

        const ruleNodeComposite = makeRuleNodeComposite(ruleNodeLeaf1, ruleNodeLeaf2, Combinations.XOR);
        console.log(await this.storeApi.addRuleToStorePolicy("a", ruleNodeComposite));

        const discountNodeLeaf = makeDiscountNodeLeaf(ruleNodeComposite, 10);
        console.log(await this.storeApi.addDiscountToStore("a", discountNodeLeaf));

        const discountCompositeNode = makeDiscountCompositeNode(discountNodeLeaf, discountNodeLeaf, Combinations.XOR);
        console.log(await this.storeApi.addDiscountToStore("a", discountCompositeNode));
        console.log("==========");
    }

    
    async getAllBidsExample() {
        console.log(await this.storeApi.getAllBidWaiting("S1"))
    }
    async componentDidMount() {
        const userBasicInfo = await this.userApi.getUserBasicInfo();
        console.log(`user: ${userBasicInfo.username} role: ${userBasicInfo.userRole}`);
        if(userBasicInfo.userRole !== UserRole.Guest){
            await this.connectToWebSocket();

            console.log(await this.getAllBidsExample());
        }

        const fetchedOwnedStoredList = await this.userApi.getAllOwnedStoreIds()
        const fetchedManagedStoredList = await this.userApi.getAllManagedStoreIds()

        if(userBasicInfo) {

            if (fetchedOwnedStoredList && fetchedOwnedStoredList.isSuccess) {
                if (fetchedManagedStoredList && fetchedManagedStoredList.isSuccess) {
                    console.log(fetchedManagedStoredList);
                    this.setState({
                        isLoggedIn: userBasicInfo.isLoggedIn,
                        userName: userBasicInfo.username,
                        role: userBasicInfo.userRole,
                        ownedStoreList: fetchedOwnedStoredList.value,
                        managedStoreList: fetchedManagedStoredList.value

                    })}
                else{
                    this.setState({
                        isLoggedIn: userBasicInfo.isLoggedIn,
                        userName: userBasicInfo.username,
                        role: userBasicInfo.userRole,
                        ownedStoreList: fetchedOwnedStoredList.value,
                    })
                }
            }
            else{
                this.setState({
                    isLoggedIn: userBasicInfo.isLoggedIn,
                    userName: userBasicInfo.username,
                    role: userBasicInfo.userRole
                })
            }


        }

    }

    async componentDidUpdate(prevProps,prevState) {
        if(!prevState.isLoggedIn && this.state.isLoggedIn && !this.state.webSocketConnection){
            //console.log("componentDidUpdate")
            await this.connectToWebSocket();
        }

        if(prevState.userName !== this.state.userName){
            if(this.state.isLoggedIn){
                const fetchedOwnedStoredList = await this.userApi.getAllOwnedStoreIds()
                const fetchedManagedStoredList = await this.userApi.getAllManagedStoreIds()
                if (fetchedOwnedStoredList && fetchedOwnedStoredList.isSuccess) {
                    if (fetchedManagedStoredList && fetchedManagedStoredList.isSuccess) {
                        this.setState({
                            managedStoreList: fetchedManagedStoredList.value,
                            ownedStoreList: fetchedOwnedStoredList.value
                        })
                    }
                    else {
                        this.setState({
                            ownedStoreList: fetchedOwnedStoredList.value
                        })
                    }
                }
                else if (fetchedOwnedStoredList && fetchedOwnedStoredList.isSuccess) {
                    this.setState({
                        managedStoreList: fetchedManagedStoredList.value
                    })
                }

            }
        }
    }

    render () {
        return (
            <BrowserRouter>
                <Layout logoutHandler={this.updateLogoutHandler} state={this.state}>
                    <Route exact path='/' component={Home} />
                    <Route exact path='/login' component={() => <Login isLoggedIn={this.state.isLoggedIn} loginUpdateHandler={this.updateLoginHandler}/>} />

                    <Route exact path='/register' component={Register}/>

                    <Route exact path='/cart' component={Cart} />
                    <Route exact path='/purchaseCart' component={() => <PurchaseCart username={this.state.userName}/>} />

                    <Route exact path="/store/:id" render={({match}) => (<Store  storeId={match.params.id}
                                                                                 ownedStoreList={this.state.ownedStoreList} redirect={this.redirectToHome}/>
                    )} />
                    <Route exact path='/openStore' component={() => <OpenStore addStoreToState={this.addOwnedStoreHandler}/>} />
                    <Route exact path="/store/:id/addItem" render={({match}) => <AddItem storeId ={match.params.id}/>} />
                    <Route exact path="/store/:id/editItem/:itemId" render={({match}) => <EditItem storeId ={match.params.id} itemId ={match.params.itemId}/>} />
                    <Route exact path="/purchaseHistory/:storeId?/:userId?/:isAdmin?" render={({match}) => <PurchaseHistory storeId ={match.params.storeId} userId={match.params.userId} isAdmin={match.params.isAdmin}/>} />
                    <Route exact path="/AdminPurchaseHistory/:term" render={({match}) => <AdminPurchaseHistory term={match.params.term}/>}/>
                    <Route exact path="/addPolicy/:storeId" render={({match}) => <AddDiscount storeId={match.params.storeId}/>}/>


                    <Route exact path="/addRule/:storeId" render={({match}) => <AddRule storeId={match.params.storeId}/>}/>

                    <Route exact path="/searchItems/Item/:query" render={({match}) => <ItemSearchDisplay itemQuery={match.params.query}  />} />
                    <Route exact path="/searchItems/Store/:query" render={({match}) => <Store  storeId={match.params.query}/>} />

                    <Route exact path="/managePermissions/:id/appointManager" render={({match}) => <AppointManager storeId ={match.params.id}/>} />

                    <Route exact path="/managePermissions/:id/appointOwner" render={({match}) => <AppointOwner storeId ={match.params.id}/>} />

                    <Route exact path="/managePermissions/:storeId/" render={({match}) => <ManagePermissions storeId ={match.params.storeId}/>}/>

                    <Route exact path="/showStats" render={({match}) => <ShowStatsInput storeId ={match.params.storeId}/>}/>

                    <Route exact path="/showStats/:date" render={({match}) => <ShowStatsOutput date ={match.params.date}/>}/>
                    
                    <Route exact path="/removeManager/:storeId" render={({match}) => <RemoveManager storeId ={match.params.storeId}/>}/>

                    <Route exact path="/removeOwner/:storeId" render={({match}) => <RemoveOwner storeId ={match.params.storeId}/>}/>

                    <Route exact path="/bidItem/:storeId/:itemId/" render={({match}) => <BidItem storeId ={match.params.storeId} itemId ={match.params.itemId} />}/>

                    <Route exact path="/showBids/:storeId" render={({match}) => <ShowBids storeId ={match.params.storeId}  />}/>


                </Layout>
            </BrowserRouter>
        );
    }
}
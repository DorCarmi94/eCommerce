import React, {Component} from "react";
import {Table} from 'react-bootstrap'
import {StatsApi} from "../Api/StatsApi";
import {HttpTransportType, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {makeTuple} from "../Data/LoginStats";

export default class ShowStatsOutput extends Component {
    static displayName = ShowStatsOutput.name;

    constructor(props) {
        super(props)
        this.state = {
            stats:[],
            date: undefined,
            webSocketConnection: undefined
        }
        this.StatsApi = new StatsApi()
    }
    

    async componentDidMount() {
        const fetchedStats = await this.StatsApi.loginStats(this.props.date)
        console.log("stats")
        console.log(fetchedStats)
        if (fetchedStats && fetchedStats.isSuccess) {
            this.setState({
                stats: fetchedStats.value.stat,
                date: this.props.date
            })
            const givenDate = this.props.date.split('-');
            const date = new Date()
            if(date.getDate() === parseInt(givenDate[1]) &&
            date.getMonth() + 1 === parseInt(givenDate[0]) &&
            date.getFullYear() === parseInt(givenDate[2])){
                await this.connectToWebSocket()
            }
        }
    }

    async connectToWebSocket(){
        const socketConnection = new HubConnectionBuilder()
            .configureLogging(LogLevel.Debug)
            .withUrl("https://localhost:5001/statsBroadcaster", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .build();

        //console.log("socketConnection")

        await socketConnection.start();
        if(socketConnection) {
            //console.log("socketConnection on")

            socketConnection.on("LoginUpdate", message => {
                const stats = this.state.stats.filter(s => s.item1 !== message.userType)
                    .concat([makeTuple(message.userType, message.amount)])
                this.setState({
                    stats: stats
                });
            });
        }
        
        this.setState({
            webSocketConnection: socketConnection
        })
    }
    
    componentWillUnmount() {
        this.setState({
            webSocketConnection: undefined
        })
    }

    render() {
        const {stats, date} = this.state
            return (
                date ?
                <div style={{marginTop: "10px"}}>
                    <h3>Statistics for date {date}</h3>
                    <Table striped bordered hover>
                        <thead>
                        <tr>
                            <th>User Type</th>
                            <th>Amount</th>
                        </tr>
                        </thead>
                        <tbody>

                        {
                            stats.map((stat) => {
                                return (
                                    <tr>
                                        <td>{stat.item1}</td>
                                        <td>{stat.item2}</td>
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
                </div> :
                    <h2>Empty stats</h2>
            );
    }
}

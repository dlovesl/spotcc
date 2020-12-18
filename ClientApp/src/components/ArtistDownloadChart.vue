<template>
  <div class="single-artist">
    <div class="columns">
          <div class="column">
            <v-select v-model="selected"
              :options="options" :reduce="t => t.name" label="name" :value="selected"
              placeholder="Please select an Artist">
            <!-- <option disabled value="">Please select one</option>
              <option v-for="option in options" :key="option.id" v-bind:value="option.name">
                {{ option.name }}
              </option> -->
          </v-select>
          </div>
           <div class="column">
             <b-field>
                <b-datepicker
                    placeholder="Click to select date range"
                    v-model="dates"
                    range>
                </b-datepicker>
            </b-field>
          </div>
          <div class="column">
              <input class="button is-primary" type="submit" @click.prevent="onChange" value="Submit"/>
          </div>
      </div>
    <!-- <label class="label">Total Streams: {{this.totalStreams}}</label> -->
    <div>
  <div class="vtc-downloads">

    <div class="vtc-downloads-controls">
      <div class="vtc-downloads-control">
      </div>
     
    </div>
    <trend-chart v-if="dataset" :datasets="[{data: dataset}]" :labels="{
        xLabels: xLabels,
        yLabels: 7,
        yLabelsTextFormatter: val =>Math.round(val)
      }" :min="0" :grid="{
        verticalLines: true,
        verticalLinesNumber: 5,
        horizontalLines: true,
        horizontalLinesNumber: 5
      }" />
  </div>
    </div>
  </div>
</template>

<script>
//import ChartDownload from "./ChartDownload.vue";
import axios from "axios";
import moment from 'moment';
import "vue-select/dist/vue-select.css";

export default {
  components: {  },
  data() {
    return {
      data: null,
      selected: '',
      options: [],
      accountId:'',
      totalStreams: 0,
      accounts: [],
      downloads: null,
      dateFormat: "DD/MM",
      dates: null
    };
  },
  computed: {
    dataset() {
      return this.downloads && this.downloads.length
        ? this.downloads.map(d => d.downloads)
        : null;
    },
    xLabels() {
      return this.downloads && this.downloads.length
        ? this.downloads.map(d => moment(d.day).format(this.dateFormat))
        : [];
    }
  },
  methods: {
    onAccountChange() {
      console.log(this.accountId);
      this.$http
        .get(`http://78.141.232.110/lq1ss/api/artist/account/` + this.accountId)
        .then((res) => {
          this.options = res.data;
          this.selected = '';
        })
        .catch((error) => console.log(error));
    },
    onChange() {
      console.log(this.selected);
      console.log(this.dates);
      if(this.dates == null) return;
      const pdata = { from: this.dates[0], to: this.dates[1], name:  this.selected};
      axios
        .post(`http://78.141.232.110/lq1ss/api/accountstream/DateRange`, pdata)
        .then(res => {
          console.log(res);
          //const { downloads } = res.data;
          console.log(res.data);
          this.downloads = res.data;
        })
        .catch(error => {
          console.log(error);
        });
    },
    onRangeChange() {
      this.onChange();
    },
    fetchData() {
      this.$http
        .get(`http://78.141.232.110/lq1ss/api/artist/getall`)
        .then((res) => {
          this.options = res.data;
          this.selected = '';
        })
        .catch((error) => console.log(error));
    }
  },
  watch: {
    //   selectedDateRange() {
    //   this.onChange();
    // }
  },
  mounted() {
    //this.selectedDateRange = this.dateRanges[0];
    this.fetchData();
  }
};
</script>

<style lang="scss">
.margin-top {
  margin-top: 15px;
}

.margin-bottom {
  margin-bottom: 15px;
}
.margin-left {
  margin-left: 5px;
}

.vs__dropdown-toggle{
  height: 40px;
}

* {
  box-sizing: border-box;
}

body {
  padding: 0;
  margin: 0;
  font-family: "Source Sans Pro", sans-serif;
  color: #2f4053;
}

#app {
  margin: 0 auto;
  padding: 20px;
  max-width: 800px;
}

.vtc-downloads {
  &-controls {
    @media (min-width: 699px) {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
    select {
      margin-left: 10px;
      font-size: 14px;
      border: 1px solid rgba(#000, 0.2);
      background: transparent;
    }
  }
  &-control {
    display: flex;
    align-items: center;
    margin-bottom: 20px;
    @media (max-width: 698px) {
      justify-content: space-between;
    }
  }

  .vtc {
    height: 250px;
    font-size: 12px;
    .stroke {
      stroke: #39af77;
      stroke-width: 2;
    }
  }

  .x-labels .label {
    font-size: 8px;
  }
}
</style>
